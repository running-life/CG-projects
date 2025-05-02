#version 330 core
in vec3 fPosition;
in vec3 fNormal;

out vec4 color;


// meterial
struct Material {
    vec3 ka;
    vec3 kd;
    vec3 ks;
    float ns;
};

// ambient
struct AmbientLight {
    vec3 color;
    float intensity;
};

// direction light
struct DirectionalLight {
    vec3 direction;
    float intensity;
    vec3 color;
};

// spot light
struct SpotLight {
    vec3 position;
    vec3 direction;
    float intensity;
    vec3 color;
    float angle;
    float kc;
    float kl;
    float kq;
};

uniform Material material;
uniform AmbientLight ambientLight;
uniform DirectionalLight directionalLight;
uniform SpotLight spotLight;
uniform vec3 cameraPosition;

// vec3 reflect(vec3 I, vec3 N) {
//     return I - 2.0 * dot(N, I) * N;
// }

vec3 calcDirectionalLight(vec3 normal, vec3 viewDir) {
    vec3 lightDir = normalize(-directionalLight.direction);
    // vec3 ambient = directionalLight.color * directionalLight.intensity * 
    //                 material.ka;
    vec3 diffuse = directionalLight.color * max(dot(normal, lightDir), 0.0f) * 
                    directionalLight.intensity * material.kd;
    vec3 specular = directionalLight.color * 
                    pow(max(dot(reflect(-lightDir, normal), viewDir), 0.0f), material.ns) * 
                    directionalLight.intensity * material.ks;
    return diffuse + specular;
}

vec3 calcSpotLight(vec3 normal, vec3 viewDir) {
    vec3 lightDir = normalize(spotLight.position - fPosition);
    float theta = acos(dot(lightDir, normalize(-spotLight.direction)));
    if (theta > spotLight.angle) {
        return vec3(0.0f, 0.0f, 0.0f);
    }
    // vec3 ambient = spotLight.color * spotLight.intensity * material.ka;

    vec3 diffuse = spotLight.color * max(dot(normal, lightDir), 0.0f) * 
                    spotLight.intensity * material.kd;

    vec3 specular = spotLight.color *
                    pow(max(dot(reflect(lightDir, normal), viewDir), 0.0f), material.ns) * 
                    spotLight.intensity * material.ks;

    float distance = length(spotLight.position - fPosition);
    float attenuation = 1.0f / (spotLight.kc + spotLight.kl * distance + 
                        spotLight.kq * distance * distance);
    return attenuation * (diffuse + specular);
}

void main() {
    vec3 normal = normalize(fNormal);
    vec3 viewDir = normalize(cameraPosition - fPosition);
    vec3 ambient = material.ka * ambientLight.color * ambientLight.intensity;
    vec3 phongColor = calcDirectionalLight(normal, viewDir) + calcSpotLight(normal, viewDir) + ambient;
    color = vec4(phongColor, 1.0f);
}