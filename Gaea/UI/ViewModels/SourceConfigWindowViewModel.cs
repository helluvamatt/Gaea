using Gaea.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaea.UI.ViewModels
{
	internal class SourceConfigWindowViewModel : LocalizableViewModel
	{
		public SourceConfigWindowViewModel(ILocalizationService l10nService, IConfiguration configService) : base(l10nService)
		{
			ConfigService = configService;
		}

		public IConfiguration ConfigService { get; private set; }
	}
}
