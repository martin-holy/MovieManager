using MH.Utils;
using MH.Utils.BaseClasses;
using MH.Utils.DB.Repositories;
using MH.Utils.Extensions;
using MovieManager.Plugins.Common.DTOs;
using PictureManager.Common.Features.Keyword;
using PictureManager.Common.Features.MediaItem;
using System;
using System.Linq;
using PM = PictureManager.Common;

namespace MovieManager.Common.Features.Movie;

public sealed class MovieR : Repository<MovieM> {
  private readonly CoreR _coreR;

  public MovieDS DataSource { get; }

  public MovieR(CoreR coreR, PM.CoreR pmCoreR) {
    _coreR = coreR;
    DataSource = new(coreR, pmCoreR, this);
  }

  public event EventHandler<MovieM[]> MoviesKeywordsChangedEvent = delegate { };
  public event EventHandler<MovieM> PosterChangedEvent = delegate { };

  public MovieM ItemCreate(MovieDetail md) {
    var item = ItemCreate(new MovieM(GetNextId(), md.Title) {
      Year = md.Year,
      YearEnd = md.YearEnd,
      Length = md.Runtime,
      Rating = md.Rating,
      MPAA = md.MPAA,
      Plot = md.Plot
    });

    item.Genres = md.Genres?
      .Select(x => _coreR.Genre.GetGenre(x, true))
      .Where(x => x != null)
      .Select(x => x!)
      .ToList() ?? [];

    return item;
  }

  public void AddMediaItems(MovieM movie, MediaItemM[] mediaItems) {
    movie.MediaItems ??= [];
    movie.MediaItems.AddRange(mediaItems.Except(movie.MediaItems));
    IsModified = true;
  }

  public void RemoveMediaItems(MovieM movie, MediaItemM[] mediaItems) {
    if (movie.MediaItems == null) return;

    foreach (var mi in mediaItems)
      movie.MediaItems.Remove(mi);

    IsModified = true;
  }

  public void AddSeenDate(MovieM movie, DateOnly date) {
    movie.Seen.AddInOrder(date, (a, b) => a.CompareTo(b));
    IsModified = true;
  }

  public void RemoveSeenDate(MovieM movie, DateOnly date) {
    movie.Seen.Remove(date);
    IsModified = true;
  }

  public void SetPoster(MovieM movie, MediaItemM mi) {
    movie.Poster = mi;
    AddMediaItems(movie, [mi]);
    IsModified = true;
    PosterChangedEvent(this, movie);
  }

  public void OnMediaItemDeleted(MediaItemM mi) {
    foreach (var movie in All.Where(x => ReferenceEquals(x.Poster, mi))) {
      movie.Poster = null;
      IsModified = true;
    }

    foreach (var movie in All.Where(x => x.MediaItems?.Contains(mi) == true)) {
      movie.MediaItems!.Remove(mi);
      IsModified = true;
    }
  }

  public void OnKeywordDeleted(KeywordM keyword) =>
    ToggleKeyword(All.Where(x => x.Keywords?.Contains(keyword) == true).ToArray(), keyword);

  public void ToggleKeyword(MovieM[] movies, KeywordM keyword) {
    foreach (var movie in movies) {
      movie.Keywords = movie.Keywords.Toggle(keyword);
      IsModified = true;
    }

    MoviesKeywordsChangedEvent(this, movies);
  }
}