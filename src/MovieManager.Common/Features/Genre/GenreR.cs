using MH.Utils.DB.Repositories;
using System;
using System.Linq;

namespace MovieManager.Common.Features.Genre;

public sealed class GenreR : Repository<GenreM> {
  public GenreDS DataSource { get; }

  public GenreR(CoreR coreR) {
    DataSource = new(coreR, this);
  }

  public GenreM? GetGenre(string name, bool create) =>
    string.IsNullOrEmpty(name)
      ? null
      : All.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? (create ? ItemCreate(new(GetNextId(), name)) : null);
}