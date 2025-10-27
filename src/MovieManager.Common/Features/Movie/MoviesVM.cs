using MH.UI.Controls;
using System.Collections.Generic;
using System.Linq;

namespace MovieManager.Common.Features.Movie;

public sealed class MoviesVM : MovieCollectionView {
  public void Open(List<MovieM> items) {
    foreach (var movie in items.Where(x => x.Poster != null))
      movie.Poster!.SetThumbSize();

    Reload(items, GroupMode.ThenByRecursive, null, true, true);
  }
}