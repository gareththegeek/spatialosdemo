# SpatialOS Demonstration
---

Based upon the SpatialOS Start Project:

- *GitHub repository*: [https://github.com/spatialos/StarterProject](https://github.com/spatialos/StarterProject)

---

## Introduction

This is a simple project I built using Unity and SpatialOS for a presentation

The project is an agent based simulation of vehicles traveling in a circle.  Each vehicle has a limited sensor range and attempts to avoid collisions with other vehicles by modifying its speed.  It is possible to specify multiple scenarios by implementing the ISnapshot interface.

The project contains a number of tags:

## 1. Starting Point

Pretty much the Starter Template with the addition of a simple Vehicle Prefab.

## 2 Simple Positioning

A simple VehicleController script causes all vehicles to travel at constant speed in a perfect circle.

## 3 Basic Vehicle Control

A VehicleControl component specifies the desired speed and maximum speed and acceleration of a vehicle.  Vehicles can only modify their speed by at most, their maximum acceleration value.

## 4 Sensors and Random Properties

Vehicles now have a sensor area and will only respond to vehicles within that area.  If no vehicles are sensed, the vehicle accelerates to maximum speed.  Otherwise the vehicle modifies its speed based upon the proximity of the nearest vehicle.  The snapshot now contains randomised maximum speed and acceleration values to make things interesting.

## 5 Brake Event

Add brake lights to the vehicle prefab and show brake likes using SpatialOS events when the vehicle is stationary or decelerating rapidly.

## 6 Snapshots (and more)

Allow multiple snapshots to be defined using the ISnapshot interface.  The way the vehicle responds to other vehicles is added to the VehicleControl component and can now be configured in snapshot.  Added a reaction time, vehicles' decisions are placed in a ring buffer introducing a delay between decision and action.

Improved sensor logic to make it compatible with multiple workers

Add a traffic light, just for fun.

## 7 Multiple Workers and Cloud Deployment

Update the default_launch.json to utilise four workers in a hex grid layout.

## Running the project

To run the project locally, first build it by running `spatial worker build`, then start the server with `spatial local launch`. You can connect a client by opening the Unity project and pressing the play button, or by running `spatial local worker launch UnityClient default`. See the [documentation](https://spatialos.improbable.io/docs/reference/latest/developing/local/run) for more details.

To deploy the project to the cloud, first build it by running `spatial worker build -t=deployment`, then upload the assembly with `spatial cloud upload <assembly name>`, and finally deploy it with `spatial cloud launch <assembly name> <launch configuration file> <deployment name> --snapshot=<snapshot file>`. You can obtain and share links to connect to the deployment from the [console](http://console.improbable.io/projects). See the [documentation](https://spatialos.improbable.io/docs/reference/latest/developing/deploy-an-application) for more details.
