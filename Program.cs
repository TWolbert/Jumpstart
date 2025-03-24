﻿using PapenChat.Framework;

namespace PapenChat {
    public class Program {
        public static void Main(string[] args) {
            // Listen on 8000 using TCPListener
            var server = new Server("0.0.0.0", 8000);
            server.Start();
        }
    }
}
