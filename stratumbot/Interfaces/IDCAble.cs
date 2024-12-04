using stratumbot.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Interfaces
{
	internal interface IDCAble
	{
		event DCAStepChangedDelegate DCAStepChangedEvent;
	}
}
