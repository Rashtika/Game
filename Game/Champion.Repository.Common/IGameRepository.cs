using Game.Common;
using Game.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Repository.Common
{
    public interface IGameRepository
    {
        Task Insert(Champion champion);
        Task<IEnumerable<Champion>> GetAll();
        Task<Champion> GetChampion(Guid id);
        Task<IEnumerable<Champion>> GetFilteredChampion(ChampionFiltering filtering, ChampionSorting sorting, ChampionPaging paging);
        Task UpdateChampion(Champion champion);
        Task DeleteChampion(Guid id);

    }
}
