﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class DialogueTree
    {
        public string Name { get; set; }
        public IList<DialogueNode> Dialogue { get; set; }
    }
}