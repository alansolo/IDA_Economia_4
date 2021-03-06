﻿using System;

namespace YahooFinance.NET
{
	public class YahooHistoricalPriceData
	{
		public DateTime Date { get; set; }
		public decimal Open { get; set; }
		public decimal High { get; set; }
		public decimal Low { get; set; }
		public decimal Close { get; set; }
		public long Volume { get; set; }
		public decimal AdjClose { get; set; }
        public double Rendimiento { get; set; }
	}
}
