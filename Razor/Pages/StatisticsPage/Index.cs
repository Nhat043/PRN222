using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Datas;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Razor.Pages
{
    public class StatisticsPageModel : PageModel
    {
        public string RevenueChartJson { get; set; } = "{}";
        public string OrdersChartJson { get; set; } = "{}";
        public string BestProductsChartJson { get; set; } = "{}";
        public string AovChartJson { get; set; } = "{}";

        private readonly DemoContext _context;
        public StatisticsPageModel(DemoContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            // Get all order items with related data
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.ProductItem)
                    .ThenInclude(pi => pi.Product)
                .ToListAsync();

            // Revenue Over Time (by month)
            var revenueByMonth = orderItems
                .Where(oi => oi.Order != null && oi.ProductItem != null)
                .GroupBy(oi => new { oi.Order.Date.Year, oi.Order.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = $"{g.Key.Year}-{g.Key.Month:00}",
                    Revenue = g.Sum(oi =>
                    {
                        var selling = oi.ProductItem.SellingPrice ?? 0;
                        var import = oi.ProductItem.ImportPrice ?? 0;
                        var discount = oi.ProductItem.Discount ?? 0;
                        var price = (int)Math.Round(selling * (1 - (double)discount/100));
                        return (price - import) * oi.Quantity;
                    })
                })
                .ToList();
            RevenueChartJson = JsonSerializer.Serialize(new {
                type = "line",
                data = new {
                    labels = revenueByMonth.Select(x => x.Label),
                    datasets = new[] {
                        new {
                            label = "Revenue",
                            data = revenueByMonth.Select(x => x.Revenue),
                            borderColor = "#007bff",
                            backgroundColor = "rgba(0,123,255,0.1)",
                            fill = true
                        }
                    }
                },
                options = new {
                    responsive = true,
                    plugins = new { legend = new { display = true } }
                }
            });

            // Orders Per Month
            var ordersByMonth = orderItems
                .Where(oi => oi.Order != null)
                .GroupBy(oi => new { oi.Order.Date.Year, oi.Order.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = $"{g.Key.Year}-{g.Key.Month:00}",
                    Orders = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .ToList();
            OrdersChartJson = JsonSerializer.Serialize(new {
                type = "bar",
                data = new {
                    labels = ordersByMonth.Select(x => x.Label),
                    datasets = new[] {
                        new {
                            label = "Orders",
                            data = ordersByMonth.Select(x => x.Orders),
                            backgroundColor = "#28a745"
                        }
                    }
                },
                options = new {
                    responsive = true,
                    plugins = new { legend = new { display = false } }
                }
            });

            // Best-Selling Products (by quantity)
            var bestProducts = orderItems
                .Where(oi => oi.ProductItem != null && oi.ProductItem.Product != null)
                .GroupBy(oi => oi.ProductItem.Product.Name)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Take(7)
                .Select(g => new
                {
                    Product = g.Key,
                    Quantity = g.Sum(oi => oi.Quantity)
                })
                .ToList();
            BestProductsChartJson = JsonSerializer.Serialize(new {
                type = "bar",
                data = new {
                    labels = bestProducts.Select(x => x.Product),
                    datasets = new[] {
                        new {
                            label = "Sold",
                            data = bestProducts.Select(x => x.Quantity),
                            backgroundColor = "#ffc107"
                        }
                    }
                },
                options = new {
                    responsive = true,
                    plugins = new { legend = new { display = false } }
                }
            });

            // Average Order Value (AOV) by month
            var aovByMonth = orderItems
                .Where(oi => oi.Order != null)
                .GroupBy(oi => new { oi.Order.Date.Year, oi.Order.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = $"{g.Key.Year}-{g.Key.Month:00}",
                    AOV = g.Select(oi => oi.Order).Distinct().Average(o => (double?)o.Price) ?? 0
                })
                .ToList();
            AovChartJson = JsonSerializer.Serialize(new {
                type = "line",
                data = new {
                    labels = aovByMonth.Select(x => x.Label),
                    datasets = new[] {
                        new {
                            label = "Avg Order Value",
                            data = aovByMonth.Select(x => x.AOV),
                            borderColor = "#e83e8c",
                            backgroundColor = "rgba(232,62,140,0.1)",
                            fill = true
                        }
                    }
                },
                options = new {
                    responsive = true,
                    plugins = new { legend = new { display = true } }
                }
            });
        }
    }
} 