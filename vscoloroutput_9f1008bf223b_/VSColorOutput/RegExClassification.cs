﻿// Copyright (c) 2011STS Software, All rights reserved
using System;
using System.Text.RegularExpressions;

namespace Simian
{
    public class RegExClassification
    {
        private string _regExPattern;

        public RegExClassification()
        {
            _regExPattern = ".*";
        }

        public string RegExPattern
        {
            get { return _regExPattern; }
            set
            {
                ValidatePattern(value);
                _regExPattern = value;
            }
        }

        public ClassificationTypes ClassificationType { get; set; }
        public bool IgnoreCase { get; set; }

        public override string ToString()
        {
            return String.Format("\"{0}\",{1},{2}", RegExPattern ?? "null", ClassificationType, IgnoreCase);
        }

        private static void ValidatePattern(string regex)
        {
            new Regex(regex);
        }
    }
}