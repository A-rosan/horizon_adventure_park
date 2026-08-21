# Horizon Adventure Park

## Business Problem

Horizon Adventure Park currently has no reliable way to:

- Confirm that a visitor is legally and safely eligible to ride a specific attraction before allowing access.
- Track how many visitors are currently occupying a ride or facility relative to its safety capacity.
- Differentiate access and pricing between visitor categories (e.g., general public, VIP guests, staff-accompanied minors).
- Track ride and staff status throughout the day (open, closed, under maintenance, assigned, unavailable).
- Prevent unsafe or invalid operations, such as exceeding ride reservation capacity or admitting a visitor without a valid, unexpired ticket.

## System Goals

The system must allow park staff to:

- Register visitors and issue tickets appropriate to their category.
- Validate whether a visitor may access a specific ride based on eligibility rules.
- Track ride status, capacity, and current occupancy.
- Manage ride reservations without exceeding safe capacity.
- Assign employees to rides or facilities and track their availability.

## Actors / Users

### Ticket Booth Staff
- Register visitors.
- Issue tickets.
- Validate tickets.

### Ride Operators
- Check ride eligibility.
- Manage ride occupancy and reservations.
- Update ride status.

### Operations Manager
- Assign employees.
- View park-wide statistics and reports.
- Manage special events.

### System
- Enforce business rules.
- Prevent invalid or unsafe actions.
- Maintain consistent state across all operations.
