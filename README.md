# GArkanoid - exercise for programming

An excersice for the Introduction to Programming course of the Games Academy 2025.

## Actions

The player can move the paddle around and is restricted to the screen size. In the beginning, the ball is attached to the paddle and waits until the player Launches it. For now it just bounces around.

### Inputs

* Moving Left - a, left arrow, joystick left
* Moving Right - d, right arrow, joystick right
* Launch Ball - Space, Gamepad south button

## Power Ups

Slow Ball
Speed Boost for the ball
Extra Life
Lose Life
Expand the paddle
Ghost Ball

## Bugs

There is a problem with going back to Main Menu from Paused state, where it keep the Game state and shows Main Menu on top of it, while the game is still playable. Possible solution is by somehow getting the previous state and clearing it, or by closing the Pause state, and going to Main Menu state from the Game state.