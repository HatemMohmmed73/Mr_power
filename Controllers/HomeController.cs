using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MR_power.Data;
using MR_power.Models;
using System.Diagnostics;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Org.BouncyCastle.Crypto;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using Microsoft.AspNetCore.Authorization;
using MR_power.Services.Interfaces;
using MR_power.DTOs;
using MR_power.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MR_power.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace MR_power.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IStockItemService _stockItemService;
        private readonly IBillService _billService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IUserService userService,
            ICustomerService customerService,
            IStockItemService stockItemService,
            IBillService billService,
            ILogger<HomeController> logger)
        {
            _userService = userService;
            _customerService = customerService;
            _stockItemService = stockItemService;
            _billService = billService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(User.IsInRole("Admin") ? "ControlPanel" : "CreateBill");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            try
            {
                var user = await _userService.AuthenticateAsync(new LoginDTO { Username = username, Password = password });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction(user.Role == "Admin" ? "ControlPanel" : "CreateBill");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ControlPanel()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                var bills = await _billService.GetAllBillsAsync();
                var totalRevenue = bills.Sum(b => b.TotalAmount);

                var viewModel = new ControlPanelViewModel
                {
                    TotalCustomers = customers.Count(),
                    TotalBills = bills.Count(),
                    TotalRevenue = totalRevenue,
                    Customers = customers.Select(c => new CustomerDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Phone = c.Phone,
                        Email = c.Email,
                        Address = c.Address
                    })
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ControlPanel");
                TempData["ErrorMessage"] = "An error occurred while loading the control panel.";
                return View(new ControlPanelViewModel());
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "CashierOrAdmin")]
        public async Task<IActionResult> CustomerPanel(string searchPhone)
        {
            try
            {
                var customers = await _customerService.SearchCustomersAsync(searchPhone ?? "");
                return View(customers.Select(c => new CustomerDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CustomerPanel");
                TempData["ErrorMessage"] = "An error occurred while loading customers.";
                return View(new List<CustomerDTO>());
            }
        }

        [Authorize(Policy = "CashierOrAdmin")]
        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [Authorize(Policy = "CashierOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateCustomerDTO customerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(customerDto);
                }

                var customer = new Customer
                {
                    Name = customerDto.Name,
                    Phone = customerDto.Phone,
                    Email = customerDto.Email,
                    Address = customerDto.Address
                };

                await _customerService.CreateCustomerAsync(customer);
                TempData["SuccessMessage"] = "Customer created successfully.";
                return RedirectToAction("CustomerPanel");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(customerDto);
            }
        }

        [Authorize(Policy = "CashierOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                return View(new UpdateCustomerDTO
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Phone = customer.Phone,
                    Email = customer.Email,
                    Address = customer.Address
                });
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "CashierOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> EditCustomer(int id, UpdateCustomerDTO customerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(customerDto);
                }

                var customer = new Customer
                {
                    Id = id,
                    Name = customerDto.Name,
                    Phone = customerDto.Phone,
                    Email = customerDto.Email,
                    Address = customerDto.Address
                };

                await _customerService.UpdateCustomerAsync(customer);
                TempData["SuccessMessage"] = "Customer updated successfully.";
                return RedirectToAction("CustomerPanel");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(customerDto);
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                return View(new CustomerDTO
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Phone = customer.Phone,
                    Email = customer.Email,
                    Address = customer.Address
                });
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost, ActionName("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomerConfirmed(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                await _customerService.DeleteAsync(customer);
                TempData["SuccessMessage"] = "Customer deleted successfully.";
                return RedirectToAction("CustomerPanel");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("CustomerPanel");
            }
        }

        [Authorize(Policy = "CashierOrAdmin")]
        public async Task<IActionResult> StockPanel(string searchTerm)
        {
            try
            {
                var stocks = await _stockItemService.SearchStockItemsAsync(searchTerm ?? "");
                return View(stocks.Select(s => new StockItemDTO
                {
                    Id = s.Id,
                    Sku = s.Sku,
                    Name = s.Name,
                    Description = s.Description,
                    Category = s.Category,
                    UnitPrice = s.UnitPrice,
                    Quantity = s.Quantity,
                    ReorderLevel = s.ReorderLevel,
                    Supplier = s.Supplier,
                    Location = s.Location,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in StockPanel");
                TempData["ErrorMessage"] = "An error occurred while loading stock items.";
                return View(new List<StockItemDTO>());
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult CreateStock()
        {
            return View();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateStock(CreateStockItemDTO stockDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(stockDto);
                }

                var stockItem = new Stook
                {
                    Sku = stockDto.Sku,
                    Name = stockDto.Name,
                    Description = stockDto.Description,
                    Category = stockDto.Category,
                    UnitPrice = stockDto.UnitPrice,
                    Quantity = stockDto.Quantity,
                    ReorderLevel = stockDto.ReorderLevel,
                    Supplier = stockDto.Supplier,
                    Location = stockDto.Location
                };

                await _stockItemService.CreateStockItemAsync(stockItem);
                TempData["SuccessMessage"] = "Stock item created successfully.";
                return RedirectToAction("StockPanel");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(stockDto);
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> EditStock(int id)
        {
            try
            {
                var stock = await _stockItemService.GetStockItemByIdAsync(id);
                return View(new UpdateStockItemDTO
                {
                    Id = stock.Id,
                    Sku = stock.Sku,
                    Name = stock.Name,
                    Description = stock.Description,
                    Category = stock.Category,
                    UnitPrice = stock.UnitPrice,
                    Quantity = stock.Quantity,
                    ReorderLevel = stock.ReorderLevel,
                    Supplier = stock.Supplier,
                    Location = stock.Location
                });
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> EditStock(int id, UpdateStockItemDTO stockDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(stockDto);
                }

                var stockItem = new Stook
                {
                    Id = id,
                    Sku = stockDto.Sku,
                    Name = stockDto.Name,
                    Description = stockDto.Description,
                    Category = stockDto.Category,
                    UnitPrice = stockDto.UnitPrice,
                    Quantity = stockDto.Quantity,
                    ReorderLevel = stockDto.ReorderLevel,
                    Supplier = stockDto.Supplier,
                    Location = stockDto.Location
                };

                await _stockItemService.UpdateStockItemAsync(stockItem);
                TempData["SuccessMessage"] = "Stock item updated successfully.";
                return RedirectToAction("StockPanel");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(stockDto);
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> DeleteStock(int id)
        {
            try
            {
                var stock = await _stockItemService.GetStockItemByIdAsync(id);
                return View(new StockItemDTO
                {
                    Id = stock.Id,
                    Sku = stock.Sku,
                    Name = stock.Name,
                    Description = stock.Description,
                    Category = stock.Category,
                    UnitPrice = stock.UnitPrice,
                    Quantity = stock.Quantity,
                    ReorderLevel = stock.ReorderLevel,
                    Supplier = stock.Supplier,
                    Location = stock.Location,
                    IsActive = stock.IsActive,
                    CreatedAt = stock.CreatedAt,
                    UpdatedAt = stock.UpdatedAt
                });
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost, ActionName("DeleteStock")]
        public async Task<IActionResult> DeleteStockConfirmed(int id)
        {
            try
            {
                await _stockItemService.DeleteStockItemAsync(id);
                TempData["SuccessMessage"] = "Stock item deleted successfully.";
                return RedirectToAction("StockPanel");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("StockPanel");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> DetailsStock(int id)
        {
            try
            {
                var stock = await _stockItemService.GetStockItemByIdAsync(id);
                return View(new StockItemDTO
                {
                    Id = stock.Id,
                    Sku = stock.Sku,
                    Name = stock.Name,
                    Description = stock.Description,
                    Category = stock.Category,
                    UnitPrice = stock.UnitPrice,
                    Quantity = stock.Quantity,
                    ReorderLevel = stock.ReorderLevel,
                    Supplier = stock.Supplier,
                    Location = stock.Location,
                    IsActive = stock.IsActive,
                    CreatedAt = stock.CreatedAt,
                    UpdatedAt = stock.UpdatedAt
                });
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize]
        public async Task<IActionResult> CreateBill()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                var stockItems = await _stockItemService.GetAllStockItemsAsync();

                ViewBag.Customers = new SelectList(customers, "Id", "Name");
                ViewBag.StockItems = new SelectList(stockItems, "Id", "Name");

                var model = new CreateBillDTO
                {
                    Date = DateTime.Now,
                    Items = new List<CreateBillItemDTO> { new CreateBillItemDTO() }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateBill GET");
                TempData["ErrorMessage"] = "An error occurred while loading the create bill page.";
                return RedirectToAction("BillPanel");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBill(CreateBillDTO billDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var stocks = await _stockItemService.SearchStockItemsAsync("");
                    ViewBag.StockItems = stocks.Select(s => new StockItemDTO
                    {
                        Id = s.Id,
                        Sku = s.Sku,
                        Name = s.Name,
                        Description = s.Description,
                        Category = s.Category,
                        UnitPrice = s.UnitPrice,
                        Quantity = s.Quantity,
                        ReorderLevel = s.ReorderLevel,
                        Supplier = s.Supplier,
                        Location = s.Location,
                        IsActive = s.IsActive,
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.UpdatedAt
                    });
                    return View(billDto);
                }

                var bill = new Bill
                {
                    CustomerId = billDto.CustomerId,
                    UserId = int.Parse(User.FindFirst("UserId").Value),
                    Date = billDto.Date,
                    Items = billDto.Items.Select(i => new BillItem
                    {
                        StockId = i.StockId,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                };

                await _billService.CreateBillAsync(bill);
                TempData["SuccessMessage"] = "Bill created successfully.";
                return RedirectToAction("BillDetails", new { id = bill.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var stocks = await _stockItemService.SearchStockItemsAsync("");
                ViewBag.StockItems = stocks.Select(s => new StockItemDTO
                {
                    Id = s.Id,
                    Sku = s.Sku,
                    Name = s.Name,
                    Description = s.Description,
                    Category = s.Category,
                    UnitPrice = s.UnitPrice,
                    Quantity = s.Quantity,
                    ReorderLevel = s.ReorderLevel,
                    Supplier = s.Supplier,
                    Location = s.Location,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                });
                return View(billDto);
            }
        }

        [Authorize(Policy = "CashierOrAdmin")]
        public async Task<IActionResult> BillPanel(string searchTerm)
        {
            try
            {
                var bills = await _billService.SearchBillsAsync(searchTerm ?? "");
                return View(bills.Select(b => new BillDTO
                {
                    Id = b.Id,
                    BillNumber = b.BillNumber,
                    Date = b.Date,
                    CustomerName = b.Customer?.Name,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BillPanel");
                TempData["ErrorMessage"] = "An error occurred while loading bills.";
                return View(new List<BillDTO>());
            }
        }

        [Authorize(Policy = "CashierOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> BillDetails(int id)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(id);
                return View(new BillDTO
                {
                    Id = bill.Id,
                    BillNumber = bill.BillNumber,
                    Date = bill.Date,
                    CustomerName = bill.Customer?.Name,
                    TotalAmount = bill.TotalAmount,
                    Status = bill.Status,
                    Items = bill.Items.Select(i => new BillItemDTO
                    {
                        StockName = i.Stock?.Name,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        Total = i.Quantity * i.Price
                    }).ToList()
                });
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "CashierOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> PrintBill(int id)
        {
            try
            {
                var pdfBytes = await _billService.GenerateBillPdfAsync(id);
                return File(pdfBytes, "application/pdf", $"bill_{id}.pdf");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("BillDetails", new { id });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;

                var bills = await _billService.GetBillsByDateRangeAsync(start, end);
                var totalSales = bills.Sum(b => b.TotalAmount);
                var lowStockItems = await _stockItemService.GetLowStockItemsAsync(10);

                ViewBag.StartDate = start;
                ViewBag.EndDate = end;
                ViewBag.TotalSales = totalSales;
                ViewBag.LowStockItems = lowStockItems.Select(s => new StockItemDTO
                {
                    Id = s.Id,
                    Sku = s.Sku,
                    Name = s.Name,
                    Description = s.Description,
                    Category = s.Category,
                    UnitPrice = s.UnitPrice,
                    Quantity = s.Quantity,
                    ReorderLevel = s.ReorderLevel,
                    Supplier = s.Supplier,
                    Location = s.Location,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                });

                return View(bills.Select(b => new BillDTO
                {
                    Id = b.Id,
                    BillNumber = b.BillNumber,
                    Date = b.Date,
                    CustomerName = b.Customer?.Name,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SalesReport");
                TempData["ErrorMessage"] = "An error occurred while generating the report.";
                return RedirectToAction("ControlPanel");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var stockItem = await _stockItemService.GetStockItemByIdAsync(id);
            if (stockItem == null)
            {
                return NotFound();
            }
            return View(stockItem);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var stockItems = await _stockItemService.SearchStockItemsAsync(searchTerm);
            return View("Index", stockItems);
        }

        public async Task<IActionResult> Category(string category)
        {
            var stockItems = await _stockItemService.GetStockItemsByCategoryAsync(category);
            return View("Index", stockItems);
        }

        public async Task<IActionResult> LowStock(int threshold = 10)
        {
            var stockItems = await _stockItemService.GetLowStockItemsAsync(threshold);
            return View("Index", stockItems);
        }
    }
}
