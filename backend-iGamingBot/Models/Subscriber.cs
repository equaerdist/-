﻿namespace backend_iGamingBot
{
    public class Subscriber
    {
        public long UserId { get; set; }
        public User? User { get; set; }
        public long StreamerId { get; set; }
        public Streamer? Streamer { get; set; }
        public DateTime SubscribeTime { get; set; }
    }
}
