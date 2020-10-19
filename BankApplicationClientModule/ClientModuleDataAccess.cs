using BankApplicationClientModule.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BankApplicationClientModule
{
    public class ClientModuleDataAccess : BaseClientModuleDataAccess
    {

        public ClientModuleDataAccess() : base()
        {
        }

        /// <summary>
        /// TODO: change this function to meet requirements.
        /// </summary>
        /// <returns></returns>
        public IList<BankClient> GetAllClientsThatHaveAtLeastOneAccountDetached()
        {
            return DBContext.BankClients.Include(x => x.ClientAccounts).Where(x => x.ClientAccounts.Any()).AsNoTracking()
                .ToList();
        }

        /// <summary>
        /// TODO: implement this function to meet requirements.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool SaveNewClient(BankClient client)
        {
            try
            {
                if (client.Id == 0)
                    DBContext.BankClients.Add(client);
                else
                    throw new DataExistsException();

                return DBContext.SaveChanges() > 0;
            }
            catch (DataExistsException e)
            {
                throw e;
            }
            catch (DbUpdateException e)
            {
                throw new WritingToDBException();
            }
        }

        /// <summary>
        /// TODO: implement this function to meet requirements.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public BankClient StartTracking(BankClient client)
        {

            if (client.Id > 0)
            {

                var objectAttached = DBContext.ChangeTracker.Entries<BankClient>().FirstOrDefault(x => x.Entity.Id == client.Id)?.Entity;
                if (objectAttached != null)
                {
                    DBContext.Entry(objectAttached).State = EntityState.Detached;
                }

                DBContext.Entry(client).State = EntityState.Modified;
                DBContext.BankClients.Update(client);
                
            }
            else
            {
                DBContext.Entry(client).State = EntityState.Added;
                DBContext.BankClients.Add(client);
            }
            DBContext.SaveChanges();

            return client;
        }

        /// <summary>
        /// TODO: implement this function to meet requirements.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsClientTrackedByEF(BankClient client)
        {
            return DBContext.ChangeTracker.Entries<BankClient>().Any(x => x.Entity.Id == client.Id);
        }
    }
}
