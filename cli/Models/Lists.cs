﻿using System;
using System.Collections.Generic;

namespace LatinAutoDeclineTester.Models
{
    public partial class Lists
    {
        public Lists()
        {
            ListsLemmas = new HashSet<ListsLemmas>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }

        public ICollection<ListsLemmas> ListsLemmas { get; set; }
    }
}