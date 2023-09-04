﻿using App.Services.Categories;
using App.Services.Posts;
using App.ViewModels.Home;

namespace App.Services.Home;

public class HomePageService : IHomePageService
{
    private readonly IPostService _postService;
    private readonly ICategoryService _categoryService;

    public HomePageService(IPostService postService, ICategoryService categoryService)
    {
        _postService = postService;
        _categoryService = categoryService;
    }

    public async Task<HomePageDto> PrepareHomeModelAsync()
    {
        var pinnedCategories = await _categoryService.GetAllPagedAsync(1, 3, onlyPinned: true);
        var pinnedPosts = await _postService.GetAllPagedAsync(1, 3, onlyPublished: true, onlyPinned: true);
        var newPosts = await _postService.GetAllPagedAsync(1, 3, onlyPublished: true);

        return new HomePageDto
        {
            PinnedCategories = pinnedCategories.Data,
            PinnedPosts = pinnedPosts.Data,
            NewPosts = newPosts.Data
        };
    }
}
