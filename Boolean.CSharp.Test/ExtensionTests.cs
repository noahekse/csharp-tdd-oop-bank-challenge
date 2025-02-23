﻿using Boolean.CSharp.Main;
using Boolean.CSharp.Main.Abstract;
using Boolean.CSharp.Main.Class;
using Boolean.CSharp.Main.Enums;
using Boolean.CSharp.Main.Interface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boolean.CSharp.Test
{
    [TestFixture]
    public class ExtensionTests
    {
        private Account _currentAccount;
        private Account _savingAccount;


        [SetUp]
        public void SetUp()
        {
            _currentAccount = new CurrentAccount(Role.Customer);
            _savingAccount = new SavingAccount(Role.Customer);
        }

        [Test]
        public void TestBalanceCalculation()
        {

            _savingAccount.Deposit(5000);
            _savingAccount.Withdraw(500);
            _savingAccount.Deposit(1000);
            _savingAccount.Withdraw(3000);

            decimal balance = _savingAccount.CalculateBalance();

            Assert.That(balance, Is.EqualTo(2500));
            
        }
        [Test]
        public void TestBranches()
        {
            _savingAccount.Branch = Main.Enums.Branch.Oslo;

            Assert.That(_savingAccount.Branch, Is.TypeOf<Main.Enums.Branch>());
            Assert.That(_savingAccount.Branch, Is.EqualTo(Main.Enums.Branch.Oslo));
        }
        [Test]
        public void TestOverdraft()
        {
            OverdraftRequest request = new OverdraftRequest(_currentAccount);

            _currentAccount.Deposit(1000);
            bool approvedOverdraft = request.Overdraft(1500);

            Assert.That(approvedOverdraft, Is.True);
        }
        [Test]
        public void TestOverdraftRequestApprovalAndRejection()
        {
            OverdraftRequest requestCurrent = new OverdraftRequest(_currentAccount);
            OverdraftRequest requestSaving = new OverdraftRequest(_savingAccount);

            _currentAccount.Deposit(1000);
            _savingAccount.Deposit(1000);
            bool currentOverdraft = requestCurrent.Overdraft(1500);
            bool savingOverdraft = requestSaving.Overdraft(1500);

            if (currentOverdraft)
            {
                requestCurrent.Approve();
            }
            else
            {
                requestCurrent.Reject();
            }

            if (savingOverdraft)
            {
                requestSaving.Approve();
            }
            else
            {
                requestSaving.Reject();
            }


            Assert.That(currentOverdraft, Is.True);
            Assert.That(savingOverdraft, Is.False);
            Assert.That(_currentAccount.CalculateBalance(), Is.EqualTo(-500));
            Assert.That(_savingAccount.CalculateBalance(), Is.EqualTo(1000));
        }
        [Test]
        public void TestStatementMessage()
        {
            _currentAccount.Deposit(1500);
            _currentAccount.Withdraw(1000);

            string message = _currentAccount.ToString();
            MessageProvider consoleMessage = new ConsoleMessage(message);
            consoleMessage.SendMessage();

            Assert.That(consoleMessage.Message, Is.TypeOf<string>());
            Assert.That(consoleMessage, Is.InstanceOf<IMessage>());
        }
    }
}
