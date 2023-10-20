using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.ViewModel.Base;
public class DialogViewModel : ViewModel
{
	public event EventHandler? DialogComplete;
	protected virtual void OnDialogComplete(EventArgs e) => DialogComplete?.Invoke(this, e);
}
