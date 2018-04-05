using System;
using System.Collections.Generic;
using System.Text;

namespace MT.DataManagement.Teams.AzureSql.Model
{
    public interface IJoinEntity<TEntity>
    {
        TEntity Navigation { get; set; }
    }
}
