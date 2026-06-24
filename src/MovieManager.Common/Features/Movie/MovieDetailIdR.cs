using MH.Utils.DB.Repositories;
using MovieManager.Plugins.Common.DTOs;
using System.Linq;

namespace MovieManager.Common.Features.Movie;

public sealed class MovieDetailIdR : Repository<MovieDetailIdM> {
  public MovieDetailIdDS DataSource { get; }

  public MovieDetailIdR(CoreR coreR) {
    DataSource = new(coreR, this);
  }

  public MovieDetailIdM ItemCreate(DetailId detailId, MovieM movie) =>
    ItemCreate(new(GetNextId(), detailId.Id, detailId.Name, movie));

  public MovieM? GetMovie(DetailId detailId) =>
    All.FirstOrDefault(x =>
      x.DetailName.Equals(detailId.Name)
      && x.DetailId.Equals(detailId.Id))?.Movie;
}