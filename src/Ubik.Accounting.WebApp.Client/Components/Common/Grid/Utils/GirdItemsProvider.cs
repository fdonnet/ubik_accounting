﻿// The .NET Foundation licenses this file to you under the MIT license.
namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils
{
    /// <summary>
    /// A callback that provides data for a <see cref="QuickGrid{TGridItem}"/>.
    /// </summary>
    /// <typeparam name="TGridItem">The type of data represented by each row in the grid.</typeparam>
    /// <param name="request">Parameters describing the data being requested.</param>
    /// <returns>A <see cref="ValueTask{GridItemsProviderResult}" /> that gives the data to be displayed.</returns>
    public delegate ValueTask<GridItemsProviderResult<TGridItem>> GridItemsProvider<TGridItem>(
        GridItemsProviderRequest<TGridItem> request);
}
