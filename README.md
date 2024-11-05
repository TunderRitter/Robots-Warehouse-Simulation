# Warehouse Robots Simulation

This repository contains a simulation program for optimizing the routes of robots in a warehouse. The simulation allows users to play out the movement of robots, experiment with different pathfinding solutions, and analyze their performance.

## Description
The simulation operates on a grid-based warehouse layout where robots navigate to assigned destinations while avoiding conflicts. The robots move in discrete time steps, following instructions from a central scheduler. They can move to adjacent cells, turn 90 degrees clockwise or counterclockwise, or remain stationary. Obstacles such as warehouse racks and walls restrict robot movement.

## Features
- **Basic Functionality**: Robots can execute tasks and manage conflicts. The program generates a log file which can be save at the end of the simulation.
- **Configurability**: Users can specify warehouse layout, number of robots, starting positions, task destinations, and scheduling strategy. The speed of the robots and the maximum number of steps are also modifiable.
- **Interactive Order Assignment**: Allows users to designate task destinations interactively during simulation.
- **Pathfinding Algorithms**: Utilizes advanced pathfinding algorithms like Cooperative A* for efficient route planning.
- **Deadlock Detection**: Detects and resolves deadlock situations to ensure smooth operation.
- **Playback and Analysis**: Log files can be loaded to replay simulations, enabling analysis and debugging.

## Usage
1. Clone the repository.
2. Configure simulation parameters. You can use the files in the sample-files folder.
3. Run the simulation program.
4. Analyze the generated log file for simulation results.


## Dependencies
- C#

## Configuration
Write a configuration file (`.json`), map file (`.map`), agents file (`.agents`) and tasks file (`.tasks`) to customize warehouse layout, robots' starting positions, tasks' positions, etc.
