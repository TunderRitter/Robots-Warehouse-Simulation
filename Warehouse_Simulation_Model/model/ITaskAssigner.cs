﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model;


public interface ITaskAssigner
{
    public void Assign(Robot[] robots, Target[] targets);
}