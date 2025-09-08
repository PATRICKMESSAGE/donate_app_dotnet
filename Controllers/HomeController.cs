using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GiveApp.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace GiveApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult Donate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Donate(DonateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // PesePay API endpoint and credentials
        var pesepayApiUrl = "https://api.pesepay.com/api/v1/payments/initiate";
        var pesepayIntegrationKey = "YOUR_PESEPAY_INTEGRATION_KEY";
        var pesepayMerchantReference = Guid.NewGuid().ToString();

        // Build the payment request payload
        var paymentRequest = new
        {
            amount = model.Amount,
            currency = "USD",
            merchantReference = pesepayMerchantReference,
            email = model.Email,
            paymentMethod = model.PaymentMethod, // e.g. "Ecocash", "Visa", "Bank"
            returnUrl = Url.Action("DonateConfirmation", "Home", null, Request.Scheme),
            resultUrl = Url.Action("DonateConfirmation", "Home", null, Request.Scheme)
        };

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pesepayIntegrationKey);

        var content = new StringContent(JsonSerializer.Serialize(paymentRequest), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(pesepayApiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            var paymentUrl = doc.RootElement.GetProperty("redirectUrl").GetString();

            // Redirect user to PesePay payment page
            return Redirect(paymentUrl);
        }
        else
        {
            ModelState.AddModelError("", "Payment initiation failed. Please try again.");
            return View(model);
        }
    }

    public IActionResult DonateConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult PesePayCallback(string status, string reference)
    {
        // You can verify payment status here using PesePay API if needed
        // For now, just redirect to confirmation
        return RedirectToAction("DonateConfirmation");
    }

    public IActionResult Welcome()
    {
        return View();
    }

    [Authorize]
    public IActionResult Dashboard()
    {
        return View();
    }
}


