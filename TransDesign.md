# Transition System Design

## First concept

My immediate idea was to add an <i>additive</i> <b>State</b> called <b>Transition</b>, which includes different animations for transitioning between scenes.

Sctructure wise, I would have to add <b>Transition</b> state as a valid state to be changed to on every other state. Make sure the scene completely covers the previous one and seemlesly change the states by the end of it's animation.

Might have to create two <b>States</b>, <b>TransitionOut</b> and <b>TransitionIn</b>, depending on how the animation will be handled.

## Second concept

It might be possible to edit the function that changes states, to make it so that it plays an animation before changing the states.