﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using ExtCore.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Platformus.Barebone.Data.Extensions;
using Platformus.Domain.Data.Abstractions;
using Platformus.Domain.Data.Entities;

namespace Platformus.Domain.Data.EntityFramework.SqlServer
{
  /// <summary>
  /// Implements the <see cref="IMemberRepository"/> interface and represents the repository
  /// for manipulating the <see cref="Member"/> entities in the context of SQL Server database.
  /// </summary>
  public class MemberRepository : RepositoryBase<Member>, IMemberRepository
  {
    public Member WithKey(int id)
    {
      return this.dbSet.AsNoTracking().FirstOrDefault(m => m.Id == id);
    }

    public Member WithClassIdAndCode(int classId, string code)
    {
      return this.dbSet.AsNoTracking().FirstOrDefault(m => m.ClassId == classId && string.Equals(m.Code, code, System.StringComparison.OrdinalIgnoreCase));
    }

    public Member WithClassIdAndCodeInlcudingParent(int classId, string code)
    {
      return this.dbSet.AsNoTracking().FromSql(
        "SELECT * FROM Members WHERE ClassId = {0} OR ClassId IN (SELECT ClassId FROM Classes WHERE Id = {0})",
        classId
      ).FirstOrDefault(m => string.Equals(m.Code, code, System.StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Member> All()
    {
      return this.dbSet.AsNoTracking().OrderBy(m => m.ClassId).ThenBy(m => m.Position);
    }

    public IEnumerable<Member> FilteredByClassId(int classId)
    {
      return this.dbSet.AsNoTracking().Where(m => m.ClassId == classId).OrderBy(m => m.Position);
    }

    public IEnumerable<Member> FilteredByClassIdInlcudingParent(int classId)
    {
      return this.dbSet.AsNoTracking().FromSql(
        "SELECT * FROM Members WHERE ClassId = {0} OR ClassId IN (SELECT ClassId FROM Classes WHERE Id = {0}) ORDER BY Position",
        classId
      );
    }

    public IEnumerable<Member> FilteredByClassIdPropertyVisibleInList(int classId)
    {
      return this.dbSet.AsNoTracking().Where(m => m.ClassId == classId && m.IsPropertyVisibleInList == true).OrderBy(m => m.Position);
    }

    public IEnumerable<Member> FilteredByClassIdInlcudingParentPropertyVisibleInList(int classId)
    {
      return this.dbSet.AsNoTracking().FromSql(
        "SELECT * FROM Members WHERE (ClassId = {0} OR ClassId IN (SELECT ClassId FROM Classes WHERE Id = {0})) AND IsPropertyVisibleInList = {1} ORDER BY Position",
        classId, true
      );
    }

    public IEnumerable<Member> FilteredByClassIdRelationSingleParent(int classId)
    {
      return this.dbSet.AsNoTracking().Where(m => m.ClassId == classId && m.IsRelationSingleParent == true).OrderBy(m => m.Position);
    }

    public IEnumerable<Member> FilteredByClassIdRange(int classId, string orderBy, string direction, int skip, int take, string filter)
    {
      return this.GetFilteredMembers(dbSet.AsNoTracking(), classId, filter).OrderBy(orderBy, direction).Skip(skip).Take(take);
    }

    public void Create(Member member)
    {
      this.dbSet.Add(member);
    }

    public void Edit(Member member)
    {
      this.storageContext.Entry(member).State = EntityState.Modified;
    }

    public void Delete(int id)
    {
      this.Delete(this.WithKey(id));
    }

    public void Delete(Member member)
    {
      this.storageContext.Database.ExecuteSqlCommand(
        @"
          DELETE FROM DataTypeParameterValues WHERE MemberId = {0};
          DELETE FROM SerializedObjects WHERE ObjectId IN (SELECT Id FROM Objects WHERE ClassId IN (SELECT ClassId FROM Members WHERE Id = {0}));
          CREATE TABLE #Dictionaries (Id INT PRIMARY KEY);
          INSERT INTO #Dictionaries SELECT StringValueId FROM Properties WHERE MemberId = {0} AND StringValueId IS NOT NULL;
          DELETE FROM Properties WHERE MemberId = {0};
          DELETE FROM Localizations WHERE DictionaryId IN (SELECT Id FROM #Dictionaries);
          DELETE FROM Dictionaries WHERE Id IN (SELECT Id FROM #Dictionaries);
          DELETE FROM Relations WHERE MemberId = {0};
        ",
        member.Id
      );

      this.dbSet.Remove(member);
    }

    public int CountByClassId(int classId, string filter)
    {
      return this.GetFilteredMembers(dbSet, classId, filter).Count();
    }

    private IQueryable<Member> GetFilteredMembers(IQueryable<Member> members, int classId, string filter)
    {
      members = members.Where(m => m.ClassId == classId);

      if (string.IsNullOrEmpty(filter))
        return members;

      return members.Where(m => m.Name.ToLower().Contains(filter.ToLower()));
    }
  }
}