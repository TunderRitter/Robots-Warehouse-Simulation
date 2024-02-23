# Warehouse Robots Simulation

This repository contains a simulation program for optimizing the routes of robots in a warehouse. The simulation allows users to play out the movement of robots, experiment with different pathfinding solutions, and analyze their performance.

## Description
The simulation operates on a grid-based warehouse layout where robots navigate to assigned destinations while avoiding conflicts. The robots move in discrete time steps, following instructions from a central scheduler. They can move to adjacent cells, turn 90 degrees clockwise or counterclockwise, or remain stationary. Obstacles such as warehouse racks and walls restrict robot movement.

## Features
- **Basic Functionality**: Robots can execute tasks, manage conflicts by waiting, and generate a log file.
- **Configurability**: Users can specify warehouse layout, number of robots, starting positions, task destinations, and scheduling strategy.
- **Interactive Order Assignment**: Allows users to designate task destinations interactively during simulation.
- **Pathfinding Algorithms**: Utilizes advanced pathfinding algorithms like A* or Cooperative A* for efficient route planning.
- **Deadlock Detection**: Detects and resolves deadlock situations to ensure smooth operation.
- **Playback and Analysis**: Log files can be loaded to replay simulations, enabling analysis and debugging.

## Usage
1. Clone the repository.
2. Configure simulation parameters in the provided configuration file.
3. Run the simulation program.
4. Analyze the generated log file for simulation results.

## Installation
Clone the repository to your local machine:

## Dependencies
- C#
- [Additional dependencies, if any]

## Configuration
Edit the configuration file (`config.json`) to customize warehouse layout, robot settings, task assignments, etc.

## Running the Simulation
Execute the simulation program with: