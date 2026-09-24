package com.example.backend.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/auth")
@CrossOrigin(origins = "http://localhost:4200")
public class AuthController {

    @PostMapping("/login")
    public ResponseEntity<String> login(@RequestBody LoginRequest request) {

        if ("user".equals(request.username()) &&
                "user123".equals(request.password())) {

            return ResponseEntity.ok("Login successful");
        }

        return ResponseEntity.status(401)
                .body("Invalid username or password");
    }

    public record LoginRequest(String username, String password) {
    }

}
