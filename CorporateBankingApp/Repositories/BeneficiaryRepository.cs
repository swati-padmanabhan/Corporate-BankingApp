﻿using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.Repositories
{
    public class BeneficiaryRepository : IBeneficiaryRepository
    {
        private readonly ISession _session;

        public BeneficiaryRepository(ISession session)
        {
            _session = session;
        }

        public Beneficiary GetBeneficiaryById(Guid id)
        {
            return _session.Get<Beneficiary>(id);
        }

        public List<Beneficiary> GetAllBeneficiaries(Guid clientId)
        {
            var beneficiaries = _session.Query<Beneficiary>().Where(b => b.Client.Id == clientId).ToList();
            return beneficiaries;
        }

        public void AddNewBeneficiary(Beneficiary beneficiary)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(beneficiary);

                //_session.Update(beneficiary.Client);
                transaction.Commit();
            }
        }

        public void EditBeneficiary(Beneficiary beneficiary)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Update(beneficiary);
                transaction.Commit();
            }
        }

        public void UpdateBeneficiaryStatus(Guid id, bool isActive)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var beneficiary = _session.Get<Beneficiary>(id);
                if (beneficiary != null)
                {
                    beneficiary.IsActive = isActive;
                    _session.Update(beneficiary);
                    transaction.Commit();
                }
            }
        }
    }
}