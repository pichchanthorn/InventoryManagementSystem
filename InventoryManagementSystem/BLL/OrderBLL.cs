using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class OrderBLL
    {
        public List<OrderEntity> GetAll(string status = null, int? customerId = null,
            DateTime? dateFrom = null, DateTime? dateTo = null, string searchText = null)
        {
            DateTime? dateToExclusive = dateTo.HasValue ? dateTo.Value.Date.AddDays(1) : (DateTime?)null;
            string normalizedSearch = NormalizeOptional(searchText);
            string normalizedStatus = NormalizeOptional(status);

            try
            {
                return OrderDAL.GetAll(normalizedStatus, customerId, dateFrom, dateToExclusive, normalizedSearch);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load order history. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load order history. Please check your database connection and try again.", ex);
            }
        }

        public List<OrderDetailEntity> GetDetails(int orderId)
        {
            try
            {
                return OrderDAL.GetDetails(orderId);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load order details. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load order details. Please check your database connection and try again.", ex);
            }
        }

        /// <summary>
        /// Creates a new Pending order. TotalAmount and each line's Total are always
        /// (re)computed here from Quantity x UnitPrice - a TotalAmount supplied by the UI
        /// is never trusted. No stock is deducted.
        /// </summary>
        public OrderResult CreateOrder(OrderEntity order, List<OrderDetailEntity> details)
        {
            if (order == null)
                return OrderResult.Fail("Order information is required.");

            if (details == null || details.Count == 0)
                return OrderResult.Fail("An order must contain at least one item.");

            int? customerId = order.CustomerID.HasValue && order.CustomerID.Value > 0 ? order.CustomerID : null;
            int? employeeId = order.EmployeeID.HasValue && order.EmployeeID.Value > 0 ? order.EmployeeID : null;

            try
            {
                if (customerId.HasValue && CustomerDAL.GetById(customerId.Value) == null)
                    return OrderResult.Fail("Selected customer does not exist.");

                if (employeeId.HasValue && EmployeeDAL.GetById(employeeId.Value) == null)
                    return OrderResult.Fail("Selected employee does not exist.");
            }
            catch (SqlException)
            {
                return OrderResult.Fail("Unable to validate the customer/employee due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return OrderResult.Fail("Unable to validate the customer/employee due to a database error. Please try again later.");
            }

            var validatedDetails = new List<OrderDetailEntity>(details.Count);
            decimal totalAmount = 0m;

            foreach (OrderDetailEntity detail in details)
            {
                if (detail.ProductID <= 0)
                    return OrderResult.Fail("Please select a valid product for every order line.");

                if (detail.Quantity <= 0)
                    return OrderResult.Fail("Quantity must be greater than zero for every order line.");

                if (detail.UnitPrice < 0)
                    return OrderResult.Fail("Unit price cannot be negative for any order line.");

                ProductEntity product;
                try
                {
                    product = ProductDAL.GetById(detail.ProductID);
                }
                catch (SqlException)
                {
                    return OrderResult.Fail("Unable to validate a product due to a database error. Please try again later.");
                }
                catch (InvalidOperationException)
                {
                    return OrderResult.Fail("Unable to validate a product due to a database error. Please try again later.");
                }

                if (product == null)
                    return OrderResult.Fail("One of the selected products no longer exists.");

                decimal lineTotal = Math.Round(detail.Quantity * detail.UnitPrice, 2, MidpointRounding.AwayFromZero);
                totalAmount += lineTotal;

                validatedDetails.Add(new OrderDetailEntity
                {
                    ProductID = detail.ProductID,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    Total = lineTotal
                });
            }

            totalAmount = Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero);

            try
            {
                int orderId = OrderDAL.CreatePendingOrder(new OrderEntity
                {
                    CustomerID = customerId,
                    EmployeeID = employeeId,
                    TotalAmount = totalAmount
                }, validatedDetails);

                return OrderResult.Ok("Order created successfully as Pending.", orderId);
            }
            catch (SqlException)
            {
                return OrderResult.Fail("Unable to save the order due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return OrderResult.Fail("Unable to save the order due to a database error. Please try again later.");
            }
        }

        /// <summary>
        /// Confirms a Pending order: deducts stock for every line item and marks the order
        /// Confirmed, atomically. Never creates a StockOut record - StockOut is a separate,
        /// already-implemented workflow, and confirming an order must not double-deduct
        /// stock through it.
        /// </summary>
        public OrderResult ConfirmOrder(int orderId)
        {
            if (orderId <= 0)
                return OrderResult.Fail("Please select an order to confirm.");

            try
            {
                OrderConfirmOutcome outcome = OrderDAL.ConfirmOrder(orderId);
                switch (outcome)
                {
                    case OrderConfirmOutcome.Success:
                        return OrderResult.Ok("Order confirmed. Stock has been deducted.", orderId);
                    case OrderConfirmOutcome.OrderNotFound:
                        return OrderResult.Fail("Order not found.");
                    case OrderConfirmOutcome.NotPending:
                        return OrderResult.Fail("Only Pending orders can be confirmed. This order may already be Confirmed or Cancelled.");
                    case OrderConfirmOutcome.NoDetails:
                        return OrderResult.Fail("This order has no items and cannot be confirmed.");
                    case OrderConfirmOutcome.InsufficientStock:
                        return OrderResult.Fail("Confirmation failed: insufficient stock for one or more products. No stock was deducted and the order remains Pending.");
                    default:
                        return OrderResult.Fail("Unable to confirm the order.");
                }
            }
            catch (SqlException)
            {
                return OrderResult.Fail("Unable to confirm the order due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return OrderResult.Fail("Unable to confirm the order due to a database error. Please try again later.");
            }
        }

        /// <summary>
        /// Cancels a Pending order. Never changes stock. Confirmed orders cannot be
        /// cancelled in Phase 6A.
        /// </summary>
        public OrderResult CancelOrder(int orderId)
        {
            if (orderId <= 0)
                return OrderResult.Fail("Please select an order to cancel.");

            try
            {
                bool cancelled = OrderDAL.CancelPendingOrder(orderId);
                if (cancelled)
                    return OrderResult.Ok("Order cancelled.", orderId);

                OrderEntity current = OrderDAL.GetById(orderId);
                if (current == null)
                    return OrderResult.Fail("Order not found.");

                return OrderResult.Fail("Only Pending orders can be cancelled. This order is currently " + current.Status + ".");
            }
            catch (SqlException)
            {
                return OrderResult.Fail("Unable to cancel the order due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return OrderResult.Fail("Unable to cancel the order due to a database error. Please try again later.");
            }
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
