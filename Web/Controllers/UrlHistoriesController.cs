﻿using App.Services.Urls;
using App.ViewModels.Urls;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Web.Controllers;

public class UrlHistoriesController : Controller
{
    private readonly IUrlHistoryService _urlHistoryService;

    public UrlHistoriesController(IUrlHistoryService urlHistoryService)
    {
        _urlHistoryService = urlHistoryService;
    }

    [Route("/{*url}", Order = 999)]
    [OutputCache(NoStore = true)]
    public async Task<IActionResult> CatchAll(string url)
    {
        var trimmedUrl = $"/{url.ToLower().TrimStart('/')}";
        var urlHistory = await _urlHistoryService.GetByUrlAsync(trimmedUrl);

        if (urlHistory is null)
        {
            return NotFound();
        }

        if (urlHistory.NewUrl is null)
        {
            return new StatusCodeResult(urlHistory.HttpStatus);
        }

        return urlHistory.HttpStatus switch
        {
            301 => new RedirectResult(urlHistory.NewUrl, permanent: false, preserveMethod: false),
            302 => new RedirectResult(urlHistory.NewUrl, permanent: true, preserveMethod: false),
            307 => new RedirectResult(urlHistory.NewUrl, permanent: false, preserveMethod: true),
            308 => new RedirectResult(urlHistory.NewUrl, permanent: true, preserveMethod: true),
            _ => throw new Exception("Error while redirecting"),
        };
    }

    [Route("Error/{code:int}")]
    [OutputCache(NoStore = true)]
    public IActionResult Error(int code)
    {
        var message = code switch
        {
            int n when 400 > n && n >= 300 => "خطا در انتقال شما به صفحه جدید رخ داده است",
            int n when 500 > n && n >= 400 => "صفحه مورد نظر شما یافت نشد",
            int n when 600 > n && n >= 500 => "خطا فنی رخ داده است",
            _ => "خطایی در سیستم رخ داده است",
        };

        var model = new ErrorDto
        {
            Message = message
        };

        return View(model);
    }
}