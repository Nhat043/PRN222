using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepo _statisticsRepo;

        public StatisticsService(IStatisticsRepo statisticsRepo)
        {
            _statisticsRepo = statisticsRepo;
        }

        public async Task<string> GetRevenueChartJsonAsync(DateTime startDate, DateTime endDate)
        {
            var orderItems = await _statisticsRepo.GetOrderItemsWithRelatedDataAsync();
            
            var revenueByMonth = orderItems
                .Where(oi => oi.Order != null && oi.ProductItem != null && oi.Order.StatusId == 2 && 
                            oi.Order.Date >= startDate && oi.Order.Date <= endDate)
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

            return JsonSerializer.Serialize(new {
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
        }

        public async Task<string> GetOrdersChartJsonAsync(DateTime startDate, DateTime endDate)
        {
            var orderItems = await _statisticsRepo.GetOrderItemsWithRelatedDataAsync();
            
            var ordersByMonth = orderItems
                .Where(oi => oi.Order != null && oi.Order.StatusId == 2 && 
                            oi.Order.Date >= startDate && oi.Order.Date <= endDate)
                .GroupBy(oi => new { oi.Order.Date.Year, oi.Order.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = $"{g.Key.Year}-{g.Key.Month:00}",
                    Orders = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .ToList();

            return JsonSerializer.Serialize(new {
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
        }

        public async Task<string> GetBestProductsChartJsonAsync(DateTime startDate, DateTime endDate)
        {
            var orderItems = await _statisticsRepo.GetOrderItemsWithRelatedDataAsync();
            
            var bestProducts = orderItems
                .Where(oi => oi.ProductItem != null && oi.ProductItem.Product != null && 
                            oi.Order != null && oi.Order.StatusId == 2 &&
                            oi.Order.Date >= startDate && oi.Order.Date <= endDate)
                .GroupBy(oi => oi.ProductItem.Product.Name)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Take(7)
                .Select(g => new
                {
                    Product = g.Key,
                    Quantity = g.Sum(oi => oi.Quantity)
                })
                .ToList();

            return JsonSerializer.Serialize(new {
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
        }

        public async Task<string> GetAovChartJsonAsync(DateTime startDate, DateTime endDate)
        {
            var orderItems = await _statisticsRepo.GetOrderItemsWithRelatedDataAsync();
            
            var aovByMonth = orderItems
                .Where(oi => oi.Order != null && oi.Order.StatusId == 2 && 
                            oi.Order.Date >= startDate && oi.Order.Date <= endDate)
                .GroupBy(oi => new { oi.Order.Date.Year, oi.Order.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = $"{g.Key.Year}-{g.Key.Month:00}",
                    AOV = g.Select(oi => oi.Order).Distinct().Average(o => (double?)o.Price) ?? 0
                })
                .ToList();

            return JsonSerializer.Serialize(new {
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