﻿using System.IO;
using MH.Utils;
using PictureManager.Plugins.Common.Interfaces.Repositories;

namespace MovieManager.Common.Repositories;

public sealed  class CoreR : SimpleDB {
  public GenreR Genre { get; }
  public MovieR Movie { get; }

  public CoreR(IPluginHostCoreR phCoreR) : base(Path.Combine("plugins", "MovieManager", "db")) {
    Genre = new(this);
    Movie = new(this, phCoreR);
  }

  public void AddDataAdapters() {
    AddTableDataAdapter(Genre);
    AddTableDataAdapter(Movie);
  }
}