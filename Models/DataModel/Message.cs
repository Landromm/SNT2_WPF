using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DataModel;

public class Message
{	
	public Message(string? hashId, string? description)
	{
		HashId = hashId;
		Description = description;
	}
	
	public string? HashId { get; set; }
	public string? Description { get; set;}

}