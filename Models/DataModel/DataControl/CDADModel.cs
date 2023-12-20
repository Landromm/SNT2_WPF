using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DataModel.DataControl;
public class CDADModel
{
	public int Temperature_ch1_CDAD { get; set; } = 0;
	public int FlowVolume_ch1_CDAD { get; set; } = 0; 
	public int FlowMass_ch1_CDAD { get; set; } = 0;
	public int Pressure_ch1_CDAD { get; set; } = 0;
	public int Temperature_ch2_CDAD { get; set; } = 0;
	public int FlowVolume_ch2_CDAD { get; set; } = 0;
	public int FlowMass_ch2_CDAD { get; set; } = 0;
	public int Pressure_ch2_CDAD { get; set; } = 0;
}
