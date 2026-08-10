using System;
using System.Collections.Generic;
using System.Text;

public class AccessTokenNotFoundException : Exception{ 
    public AccessTokenNotFoundException() 
        : base("Access token was not found.") 
    { 
    }
}
