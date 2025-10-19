# Chapeau - Restaurant Ordering System

> A staff-facing application to manage customer orders, providing efficient Create, Read, Update, and Delete (CRUD) operations and real-time status visibility.

Chapeau is a core system designed for restaurant staff, including servers and kitchen personnel, to streamline the order management process. It was developed as a group project and forked from the original [Jeroen-vd-Valk/Chapeau](https://github.com/Jeroen-vd-Valk/Chapeau) repository.

---

## ✨ Key Features

This system is built to provide staff with a clear and effective way to manage the flow of orders.

* **Order CRUD:** Easily **C**reate new customer orders, **R**ead details of existing orders, **U**pdate items or status, and **D**elete an order if necessary.
* **Real-Time Status Tracking:** Staff can instantly see the **current status** of every order (e.g., *New*, *Preparing*, *Ready for Pickup*, *Served*).
* **Intuitive UI:** A clean, web-based interface built with **HTML, CSS, and JavaScript** for fast and easy navigation on common restaurant hardware (e.g., tablets or terminals).
* **Robust Backend:** Built on **C#**, ensuring reliable processing and data handling for all transactions.

---

## 💻 Technical Stack

The core of the application utilizes a C# backend to handle business logic and data persistence, paired with a web frontend for accessibility.

* **Backend:** **C#** (Likely **ASP.NET Core** or similar)
* **Frontend:** **HTML**, **CSS**, and **JavaScript**
* **Database:** **MySQL**

---

## 🚀 Getting Started

This guide will get a copy of the project running on your local development machine.

### Prerequisites

You'll need the following installed:

* **[.NET SDK](https://dotnet.microsoft.com/download)**: The C# code requires the **[.NET VERSION, e.g., 6.0, 8.0]** SDK.
* **[DATABASE DRIVER/TOOL, if applicable]**

### Installation

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/patriciohuang/Chapeau.git](https://github.com/patriciohuang/Chapeau.git)
    cd Chapeau
    ```
2.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```
3.  **Database Setup:**
    * [STEPS TO CONFIGURE DATABASE CONNECTION STRING in a file like `appsettings.json`]
    * [COMMAND TO RUN MIGRATIONS, e.g., `dotnet ef database update`]

4.  **Run the Application:**
    ```bash
    dotnet run --project [PROJECT_NAME_OR_PATH, e.g., Chapeau.Web]
    ```
    The application will typically launch at `https://locall.host/5000/`.

---

## 👥 Group Project & Acknowledgments

This project was a collaborative effort to develop a comprehensive restaurant management system. This repository is a fork created by Patricio Huang for individual contribution tracking and project preservation.

This project was a collaborative effort by:

### Team Contributions

| Member | Primary Area of Focus | Description of Responsibilities |
| :--- | :--- | :--- |
| **Patricio Huang** | **Kitchen & Bar View** | Developed the staff-facing order viewing system, allowing chefs and bar staff to **view incoming orders**, filter orders, and mark items/orders as **completed**. |
| **Jeroen van der Valk** | **Waiter Order Management** | Managed the core order-taking system, allowing waiters to **create new orders**, and **update/modify** existing orders (CRUD functions). |
| **Diana Lagaeva** | **Payment & Billing** | Implemented the payment processing features, allowing waiters to handle **various payment methods** and providing the functionality to **split the bill** for customers. |
| **David Sebestyen** | **User Management & Frontend** | Developed the **login and authentication system** and the **employee management** module, allowing managers to create employees and assign different roles. |

### License

This project is licensed under the **MIT License**.

### Acknowledgments

* Thanks to [Jeroen-vd-Valk](https://github.com/Jeroen-vd-Valk/Chapeau) for setting up the original repository structure.
* Developed as a final project for the **[Building applications]** at **[InHOlland]**.