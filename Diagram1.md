graph LR
    A["Program.cs<br/>Console I/O"] -->|"calls method"| B["Repository<br/>.FirstOrDefault() LINQ"]
    B -->|"modifies _list"| C["SalveazaToti()<br/>writes to .txt"]