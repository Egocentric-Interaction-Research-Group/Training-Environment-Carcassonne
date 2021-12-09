# ReadMe - cARcassonne AI agent

This ReadMe file focuses specifically on the AI implementation, i.e. the code and functionality of the AI agent, not the training environment, for the cARcassonne project. 

This document is useful if you are looking into changing or expanding the functionality of the AI agent, or training new versions of the agent.

## ML-Agents
The AI agent is based on ML-agents, which is a built in unity tool for AI. It uses reinforcement learning, meaning it recieves inputs and provides outputs initally at random. Through code segments that reward or punish the agent, by increasing or decreasing its reward attribute, it learns what behaviour is wanted.
It is possible to continue to train an already trained AI, which can be useful if additional features are added, or if some of the current bugs are fixed.

## Components
The implementation essentially has four vital components that occur in the code. These components are (in the order they happen):
* Observations - Defines what the agent knows. It is collections of values from the game environment in which the AI acts.
* Decision - Defines what the agent can do. It represents the various choices it can make in different game states. 
* Action - This defines what will happen for each of the decisions that can be made in a state.
* Rewards - Indicates whether the outcome of the decision and its corresponding action was wanted or not.

This means that when the agent is requested to take action it collects all given observations that it knows of, defined in the code, and from this provides an output which is the decision. Before the agent is trained, this output is completely random from the set of possible outputs. Each outpur, or decision, that the agent can take is mapped to a specific action taking place in the game, for example placing a tile or a meeple. When an action occurs, the game state is updated. From this action in itself, or changes that took place in the game state, the agent can (but does not have to) recieve a positive or negative reward depending on whether the behviour is wanted or not. This is defined by the implementer.

## Implementation
In this section, the general structure of this agent will be outlined. An instruction for how the four components listed above is implemented and can be changed will also be included. The first section, *Structure*, will outline the three agent-related classes and their respective purposes, while the other sections will focus on the implementation of the components mentioned in the previous section. 

### Structure
While the agent itself consists of only one class, in this project called CarcassonneAgent.cs, it also utilizes two helper classes. These are rather simplistic:
* *AIDecisionRequester* - This class has two main functionalities. 
  * It checks wether or not it is the current player's (played by the agent) turn to act. 
  * If it is the agents turn to act, it either acts directly through this class with hard coded actions for certain states that only have one outocome (drawing a tile, ending a turn), or it request a decision from the agent resulting in an action. This works the same for untrained or trained agents, the only difference is the likelyhood of each action based on the observations the agent has made in the game state.
* *AIWrapper* - This class only serves the purpose of acting as a middle-man between the agent and the environment it acts in. This is done by the agent class itself calling methods in the AIWrapper to execute its actions, rather than directly in the game code. The main reason for this is the many differences in the code structure of the real cARcassonne environment and the training environment.

These two helper classes are, compared to the agent itself, rather simplistic and straight forward. They outline how the agent can act through the static hard-coded decisions and actions it can take, as well as many of the observations it recieves through the wrapper. If no change in the actual behaviour of the AI is wanted, but it is needed to change how it interacts with the code, making changes in the wrapper class should be sufficient.

The *AIDecisionRequest*-class will always act first. It decides if it is time to act, and if so either acts directly or, in the more advanced cases such as placing a tile or a meeple, requests a decision from the actual agent. The agent class *CarcassonneAgent*, if a decision is requested, collects observations from the game environment through calls to the *AIWrapper*-class. These are used by the agent to provide a decision output, which is mapped to an action call to the *AIWrapper*-class, which performs the actual actions in the game environment. Based on the action choice or the changes in the game environment, the *CarcassonneAgent* recieves positive or negative rewards of varied values.



### Observations
