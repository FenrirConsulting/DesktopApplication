using System;
using System.Collections.Generic;

namespace IAMHeimdall.Core
{
    public class Utilities
    {
        #region Properties
        //List of Words to Remove from Search Strings. Filter prevents SQL Injection
        public static readonly List<String> FilteredWords = new(new[]
  {
            "select", 
            "Select",
            "select*",
            "Select*",
            "like", 
            "Like", 
            "'", 
            "=", 
            "*", 
            "Base",
            "BASE",
            "SELECT",
            "base", 
            "select=", 
            "Select=",
            "%"
         });

        #endregion

        #region Methods
        public class PrincipalInfo
        {
            public String SAM { get; set; }
            public String Domain { get; set; }
            public String Type { get; set; }
        }
        #endregion
    }
}