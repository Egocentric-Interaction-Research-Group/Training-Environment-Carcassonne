# ReadMe - cARcassonne AI agent

This ReadMe file focuses specifically on the AI implementation, i.e. the code and functionality of the AI agent, not the training environment, for the cARcassonne project. 

This document is useful if you are looking into changing or expanding the functionality of the AI agent, or training new versions of the agent.

## ML-Agents
The AI agent is based on ML-agents, which is a built in unity tool for AI. It uses reinforcement learning, meaning it recieves inputs and provides outputs initally at random. Through code segments that reward or punish the agent, by increasing or decreasing its reward attribute, it learns what behaviour is wanted.
It is possible to continue to train an already trained AI, which can be useful if additional features are added, or if some of the current bugs are fixed.

## Components
The implementation essentially has four vital components that occur in the code. These components are (in the order they happen):
* Observations - Defines what the agent knows. It is collections of values from the game environment in which the AI acts.
* Decisions - Defines what the agent can do. It represents the various choices it can make in different game states. 
* Actions - This defines what will happen for each of the decisions that can be made in a state.
* Rewards - Indicates whether the outcome of the decision and its corresponding action was wanted or not.

This means that when the agent is requested to take action it collects all given observations that it knows of, defined in the code, and from this provides an output which is the decision. Before the agent is trained, this output is completely random from the set of possible outputs. Each outpur, or decision, that the agent can take is mapped to a specific action taking place in the game, for example placing a tile or a meeple. When an action occurs, the game state is updated. From this action in itself, or changes that took place in the game state, the agent can (but does not have to) recieve a positive or negative reward depending on whether the behviour is wanted or not. This is defined by the implementer.

## Implementation
In this section, the general structure of this agent will be outlined. An instruction for how the four components listed above is implemented and can be changed will also be included. The first section, *Structure*, will outline the three agent-related classes and their respective purposes, while the other sections will focus on the implementation of the components mentioned in the previous section. Note that all code snippets uses as examples are from 2021-12-09 and may have been modified since.

### Structure
The agent itself consists of only one class, in this project called *CarcassonneAgent*, that inherits *Agent* from the ML-Agents package. However, it also utilizes two helper classes. These are rather simplistic both in code and functionality:
* *AIDecisionRequester* - This class has two main functionalities. 
  * It checks wether or not it is the current player's (played by the agent) turn to act. 
  * If it is the agents turn to act, it either acts directly through this class with hard coded actions for certain states that only have one outocome (drawing a tile, ending a turn), or it request a decision from the agent resulting in an action. This works the same for untrained or trained agents, the only difference is the likelyhood of each action based on the observations the agent has made in the game state.
* *AIWrapper* - This class only serves the purpose of acting as a middle-man between the agent and the environment it acts in. This is done by the agent class itself calling methods in the AIWrapper to execute its actions, rather than directly in the game code. The main reason for this is the many differences in the code structure of the real cARcassonne environment and the training environment.

These two helper classes are, compared to the agent itself, rather simplistic and straight forward. They outline how the agent can act through the static hard-coded decisions and actions it can take, as well as many of the observations it recieves through the wrapper. If no change in the actual behaviour of the AI is wanted, but it is needed to change how it interacts with the code, making changes in the wrapper class should be sufficient.

The *AIDecisionRequest*-class will always act first. It decides if it is time to act, and if so either acts directly or, in the more advanced cases such as placing a tile or a meeple, requests a decision from the actual agent. The agent class *CarcassonneAgent*, if a decision is requested, collects observations from the game environment through calls to the *AIWrapper*-class. These are used by the agent to provide a decision output, which is mapped to an action call to the *AIWrapper*-class, which performs the actual actions in the game environment. Based on the action choice or the changes in the game environment, the *CarcassonneAgent* recieves positive or negative rewards of varied values.



### Observations
In ML-Agents, the observations are collected by overriding the *CollectObservations*-method from the inherited *Agent* class. This method recieves a VectorSensor as input, which is used to add observations. The observations are simply numeric value representations of various aspects of the game state that the agent needs to know about to make an informed decision. Some examples is the tile-id and rotation of each tile, including the one to place, and how many meeples the agent has left.

Implementing this in code is rather simple. The complicated work here is rather to figure out all information that is needed, and what is not needed, to make an informed decision. The observed parameters are prefered to be normalized between the values -1 to 1. The code for this in the agent is rather short:
```
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(MeeplesLeft / meeplesMax);
        sensor.AddObservation(wrapper.GetCurrentTileId() / wrapper.GetMaxTileId());
        sensor.AddObservation(rot / 3f);
        sensor.AddObservation(x / wrapper.GetMaxBoardSize());
        sensor.AddObservation(z / wrapper.GetMaxBoardSize());
        sensor.AddObservation((int)meepleDirection);
        sensor.AddObservation(wrapper.GetNumberOfPlacedTiles() / wrapper.GetTotalTiles());

        //One-Hot observations of enums (can be done with less code, but this is more readable)
        int MAX_PHASES = Enum.GetValues(typeof(Phase)).Length;
        int MAX_DIRECTIONS = Enum.GetValues(typeof(Direction)).Length;

        sensor.AddOneHotObservation((int)wrapper.GetGamePhase(), MAX_PHASES);
        sensor.AddOneHotObservation((int)meepleDirection, MAX_DIRECTIONS);

        // Call the tile observation method that was assigned at initialization,
        // using the editor-exposed 'observationApproach' field.
        AddTileObservations?.Invoke(sensor);
    }
```
Important things to note is that each observation is added separately, and that enums are added in a special way. The last line of code is a special implementation for observing the board (with many possible different ways of doing so) used when training the AI.

### Decisions
There are two types to handle decisions in ML-Agents. They can be *continous*, meaning there is a constant stream of decisions rather than single distinct decisions. For example, steering while driving would be continous. In the Carcassonne case however, we use discrete *actions*, which occurs on distinct separate occasions, as this is a board game with one distinct action per phase. Additionally, ML-Agents allows branches for several decisions, and thereby actions, to take place simultaneously. This can also be examplified by driving, where steering and acceleration could both happen at the same time. In this case it is once again more simplistic, as only one action needs to take place for any specific decision. Therefore, only one branch is used, and all possible decisions are mapped to this one branch in the code.

The code that sets the allowed number of actions can be found in the method *WriteDiscreteActionMask* which is overridden from the inherited *Agent* class. This class allows us to set how many choices are available for a single decision in the various game stages. The code is rather simple:
```
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        int allowedActions = 0;
        switch (wrapper.GetGamePhase())
        {
            case Phase.TileDrawn:
                //AI can choose to step one tile place in either of the 4 directions (-X, X, -Z, Z), rotate 90 degrees, or confirm place.
                allowedActions = 6;
                break;
            case Phase.TileDown:
                //AI can choose to take or not take a meeple.
                allowedActions = 2;
                break;
            case Phase.MeepleDrawn:
                //AI can choose to place a drawn meeple in 5 different places (N, S, W, E, C) or confirm/deny current placement.
                allowedActions = 6;
                break;
        }

        //Disables all actions of branch 0, index i (on that branch) for any i larger than the allowed actions.
        for (int i = allowedActions; i < maxBranchSize; i++)
        {
            actionMask.SetActionEnabled(0, i, false); //The rest are enabled by default, as it resets to all enabled after a decision.
        }
    }
```
In the inspector of the *AI* prefab object in unity, it is possible to set the amount of branches and max allowed actions per branch. In our case, we only have one branch (indexed as branch 0) and maximum 6 allowed actions, which is used in the TileDrawn phase and the MeepleDrawn phase, where the agent needs to place the drawn tile or meeple. In the TileDown phase however, there are only two allowed actions. The reasoning for the amount of allowed actions are as follows:
* TileDrawn - In this phase, rather than picking from the entire board combined with rotations, the newly drawn tile starts on the base tile with rotation 0, and the AI can move it one step at a time. That is, it may move one step up, down, left, or right on the board (4 actions). Additionally, it can rotate the tile 90 degrees at a time (1 action), and it can confirm placing the tile in its current position (1 action). This means that the agent is repeatedly request to make one of the 6 decisions until a valid placement is performed.
* TileDown - In this phase, there are only two actions since the agent only needs to decide whether to draw a meeple, ending up in MeepleDrawn phase, or ending the turn without drawing a meeple.
* MeepleDrawn - In this phase, the newly placed tile is the centerpiece of decisionmaking. There are 5 different places on that tile where a meeple could potentially stand (North, South, West, East, and Center), which corresponds to 5 of the possible decisions. The last action is confirming the current placement of the meeple. In the game code, an invalid placement means the meeple is returned to the hand, and thus the game returns to the TileDown phase.

Each time this method is called, which is once per requested action, the max number of allowed actions is reset to its default value in the inspector. Therefore, at the end of this method, the allowed number for this specific phase is set through a loop. These allowed decisions will directly map to the actions the agent can take.

### Actions
