﻿using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();

        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1) // view your current balance
                {
                    consoleService.DisplayBalance();
                }
                else if (menuSelection == 2) // view your past transfers
                {
                    consoleService.DisplayTransfers();
                    int transferId = consoleService.GetInteger("Enter ID of transaction to get more information (0 to cancel):");
                    if (transferId != 0)
                    {
                        consoleService.DisplayTransfer(transferId);
                    }
                }
                else if (menuSelection == 3) // view youre pending requests
                {
                    consoleService.DisplayPendingTransfers();
                    // approve/reject
                    int transferId = consoleService.GetInteger("Please enter transfer ID to approve/reject (0 to cancel):");
                    if (transferId != 0)
                    {
                        consoleService.DisplayTransfer(transferId);
                        consoleService.RespondToTransfer(transferId);
                    }
                }
                else if (menuSelection == 4) // send TE bucks
                {
                    //display users
                    consoleService.DisplayUsers();
                    //get user selection
                    int transferToID = consoleService.GetInteger("Enter ID of user you are sending to (0 to cancel):");
                    if (transferToID != 0) // allows user to break out of transfer if they enter '0'
                    {
                        decimal amount = consoleService.GetDecimal("Enter amount:");
                        API_Transfer t = new API_Transfer()
                        {
                            TransferTypeId = API_TransferTypes.Send,
                            TransferStatusId = API_TransferStatus.Approved,
                            ToUserId = transferToID,
                            Amount = amount
                        };

                        //initiate transfer
                        consoleService.CreateTransfer(t);
                    }
                }
                else if (menuSelection == 5) // request TE bucks
                {
                    //display users
                    consoleService.DisplayUsers();
                    //get user selection
                    int requestFromId = consoleService.GetInteger("Enter ID of user you are requesting from (0 to cancel):");
                    if (requestFromId != 0) // allows user to break out of transfer if they enter '0'
                    {
                        decimal amount = consoleService.GetDecimal("Enter amount:");
                        API_Transfer t = new API_Transfer()
                        {
                            TransferTypeId = API_TransferTypes.Request,
                            TransferStatusId = API_TransferStatus.Pending,
                            FromUserId = requestFromId,
                            Amount = amount
                        };

                        //initiate transfer
                        consoleService.CreateTransfer(t);
                    }
                }
                else if (menuSelection == 6) // login as different user
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else // exit
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
