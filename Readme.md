Components Description:

 -  **Gems Pooling** 
    For this purpose used x(z)Pooling asset. Added prewarm of required gems on board 
	*PoolObject* -  component, which implements logic of pooling object, which works with GenericPooler component. Used by SC_Gem.
	 *ParticlePoolObject* - component used for destroy gem particles.
 - **Coroutine Runner** - used to run all coroutines on single MonoBehaviour and do not depend on GameObject activity state.
 - **SC_Gem** - refactored, only left logic, related to gem itself (position on board, color, type)
 - **SC_GameLogic** - refactored, only left logic as main entry point, score display.
 - **GameBoard** - refactored, only left related to data logic.
 - **StrategyAnimationsData** - scriptable object, which holds animation data for fill strategy.

> Board Fill group
 - **Board filler** - main component, which invokes initial board fill and refill, also creates bomb
 - **BoardGemOrchestrator** - orchestrator, which invokes gems movement, spawn. Used by ParallelFillStrategy and SequentialFillStrategy components. Just one of implementation options, not necessary, but can be used in future or can be removed.
 -  **IBoardFillStrategy** - main inteface to fill and refill board.
 - **GemMover** - component to move gems to required positions. Process movement orders in queue.
 - **IGemPicker** - interface to pick required gem by it's own logic.
 - **IGemSpawner** - interface to spawn gem, provided by IGemPicker, on required position.
>  Board logic group
- **BoardIndexCache** - cache for faster gems lookup.
- **GemSwapController** - triggers gem swap action on board from user input
- **InputController** - handles user input
- **IMatchResolver** - interface of match resolver to handle all matches on board.
- **MatchResolver** - match resolver implementation with destroy delays during resolve and calls of board refill. Also applies bomb spawn rules.
- **MatchClustering** - collects all cross-line (T-like) matches after got scan result from MatchDetector
- **MatchData** - data models to resolve matches
- **MatchDetector** - collects all matches on board
> Bomb group
- **IBombSpawnRule** - interface rule to spawn bomb during match resolution. Has 2 rules **BombOnFourthRule** and **BombWithGemsOnLineRule**.
- **BombOnFourthRule** - spawns colored bomb of color from gems match.
- **BombWithGemsOnLineRule**- spawns colored bomb, if happened match of colored bomb and 2+ gems of same color.
- **BombActivationResult** - data of bomb activation service with targets and bomb center.
- **BombActivationService** - bomb activation, which builds reaction chain of square or diamond bomb and returns result of activation
- **BombMatchUtils**- helper for match of bomb with regular gems.
- **IColoredBombPicker** - interface to pick proper colored bomb
- **ColoredBombPicker** - implementation, which returns properly picked colored bomb.
- **IBombPlacer**- interface to place colored bomb at place
- **ColoredBombPlacer**- implementation of colored bomb spawner
	