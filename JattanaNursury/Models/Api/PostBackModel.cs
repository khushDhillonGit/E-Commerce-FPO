﻿using Microsoft.AspNetCore.Components.Web;

namespace JattanaNursury.Models.Api
{
    public class PostBackModel
    {
        public bool Success { get; set; }
        public string ResponseText { get; set; }
        public string RedirectUrl { get; set; }
    }
}
