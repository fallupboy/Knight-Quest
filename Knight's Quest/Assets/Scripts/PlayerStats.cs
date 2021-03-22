public static class PlayerStats
{
    private static int coins = 0, crystals = 0, healthPoints = 100, lives = 3;

    public static int Coins
    {
        get { return coins; }
        set { coins = value; }
    }
    
    public static int Crystals
    {
        get { return crystals; }
        set { crystals = value; }
    }

    public static int HealthPoints
    {
        get { return healthPoints; }
        set { healthPoints = value; }
    }

    public static int Lives
    {
        get { return lives; }
        set { lives = value; }
    }
}
