﻿using Blog.Models;

namespace Blog.ViewModels
{
    public class HomeVM
    {
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ThumbnailUrl { get; set; }
        public List<Post> posts { get; set; }

    }
}
