﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageLogger.Model
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Message(string content)
        {
            Content = content;
            CreatedAt = DateTime.Now.ToUniversalTime();
        }
    }
}
