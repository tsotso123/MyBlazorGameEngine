using game.Models.interfaces;

namespace game.Models.ui
{
    public class SummonUnitButton : UiButton
    {
        public GameManager Gamemanager { get; }

        private string unitToSummon;

        int counter = 0;

        Random rnd = new Random();
        ManaManager manaManager { get; }
        public SummonUnitButton(int x, int y, int width, int height, IRenderer renderer, GameManager gameManager, string unitToSummon,ManaManager manaManager) : base(x, y, width, height, renderer)
        {
            Gamemanager = gameManager;
            this.unitToSummon = unitToSummon;
            this.manaManager = manaManager;
        }
        public override void ButtonClicked()
        {
            Console.WriteLine("summoning unit:" + counter++);

            Gamemanager.PostFrameActions.Enqueue(() =>
            {
                manaManager.BuyUnit(Gamemanager.CreateUnit(unitToSummon, Gamemanager.BaseLocationX + 45, Gamemanager.BaseLocationY + 64));
                //Gamemanager.AddUnit(Gamemanager.CreateUnit(unitToSummon, Gamemanager.BaseLocationX + 45, Gamemanager.BaseLocationY + 64));
            });



        }
    }
}
