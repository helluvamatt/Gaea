using Gaea.Api.Data;
using System;
using System.Drawing;
using System.Threading;

namespace Gaea.Api
{
    public interface ISource : IDisposable
    {
		string Name { get; }

		/// <summary>
		/// Display name for the source
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Brief description of the source
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Icon representing the source
		/// </summary>
		Icon Icon { get; }

		/// <summary>
		/// Configuration object, will use reflection to determine what fields need to be displayed
		/// 
		/// Return null (or "new {}" ) if there is no configuration
		/// </summary>
		object Configuration { get; }

		/// <summary>
		/// Initialize the source
		/// 
		/// Implementations should use this to do any one-time setup
		/// </summary>
		void Initialize();

		/// <summary>
		/// Configure the source with the supplied configuration object
		/// </summary>
		/// <param name="configuration">Configuration object that was previously persisted from the Configuration property</param>
		void Configure(object configuration);

		/// <summary>
		/// Start fetching the next image
		/// </summary>
		/// <param name="cancelToken">Used for cancelling the operation</param>
		void BeginFetchNext(CancellationToken cancelToken);

		/// <summary>
		/// Attempt to cancel the currently running fetch
		/// </summary>
		void CancelFetch();

		/// <summary>
		/// The next image has been fetched and is ready to display
		/// </summary>
		event FetchNextCompleteHandler FetchNextComplete;

		/// <summary>
		/// A fetch error has occurred
		/// </summary>
		event FetchErrorHandler FetchError;
    }

	public delegate void FetchNextCompleteHandler(GaeaImage next);

	public delegate void FetchErrorHandler(string errorTitle, string errorMessage);
}
