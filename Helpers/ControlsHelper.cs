using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace LocalizeMaster.Helpers
{
    public static class ControlsHelper
    {
        public static async Task<string> OpenFolderDialog(this Window window)
        {
            var dialog = new OpenFolderDialog();
            return await dialog.ShowAsync(window);
        }

        public static async Task<string[]> OpenFileDialog(this Window window, bool allowMultiple = true, params FileDialogFilter[] filters)
        {
            var dialog = new OpenFileDialog();
            dialog.AllowMultiple = allowMultiple;
            if(filters != null && filters.Length > 0)
                dialog.Filters = filters.ToList();
            return await dialog.ShowAsync(window);
        }
    }
}
