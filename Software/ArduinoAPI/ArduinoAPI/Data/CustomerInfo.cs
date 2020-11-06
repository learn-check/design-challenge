﻿namespace ArduinoAPI.Data
{
    public class CustomerInfo
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string CardType { get; set; }
        public int StartLocation { get; set; }
        public int EndLocation { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Surname}, {CardType}, {StartLocation}, {EndLocation}";
        }
    }
}
