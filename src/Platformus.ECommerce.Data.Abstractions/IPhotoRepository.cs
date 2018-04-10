﻿// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using ExtCore.Data.Abstractions;
using Platformus.ECommerce.Data.Entities;

namespace Platformus.ECommerce.Data.Abstractions
{
  /// <summary>
  /// Describes a repository for manipulating the <see cref="Photo"/> entities.
  /// </summary>
  public interface IPhotoRepository : IRepository
  {
    /// <summary>
    /// Gets the photo by the identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the photo.</param>
    /// <returns>Found photo with the given identifier.</returns>
    Photo WithKey(int id);

    /// <summary>
    /// Gets the cover photo filtered by the product identifier.
    /// </summary>
    /// <param name="productId">The unique identifier of the product this photo belongs to.</param>
    /// <returns>Found cover photo with the given product identifier.</returns>
    Photo CoverByProductId(int productId);

    /// <summary>
    /// Gets the photos filtered by the product identifier using sorting by position (ascending).
    /// </summary>
    /// <param name="productId">The unique identifier of the product these photos belongs to.</param>
    /// <returns>Found photos.</returns>
    IEnumerable<Photo> FilteredByProductId(int productId);

    /// <summary>
    /// Creates the photo.
    /// </summary>
    /// <param name="photo">The photo to create.</param>
    void Create(Photo photo);

    /// <summary>
    /// Edits the photo.
    /// </summary>
    /// <param name="photo">The photo to edit.</param>
    void Edit(Photo photo);

    /// <summary>
    /// Deletes the photo specified by the identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the photo to delete.</param>
    void Delete(int id);

    /// <summary>
    /// Deletes the photo.
    /// </summary>
    /// <param name="photo">The photo to delete.</param>
    void Delete(Photo photo);

    /// <summary>
    /// Counts the number of the photos filtered by the product identifier with the given filtering.
    /// </summary>
    /// <param name="productId">The unique identifier of the product these photos belongs to.</param>
    /// <param name="filter">The filtering query.</param>
    /// <returns>The number of photos found.</returns>
    int Count(int productId, string filter);
  }
}